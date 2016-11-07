'use strict';
const express = require('express');
const url = require('url') ;
const bodyParser = require('body-parser');
const app = express();
const addTodo = require('./addTodo');
app.use(bodyParser);//.json());

// Server settings
const serverPort = 4730;
const serverUrl = 'localhost' + serverPort;
const dbUrl = 'mongodb://localhost:27017/punch';

// Companies and users data storage
const companies = [];
const users = [];

// Id generators
let companyId = 0;
let userId = 0;

app.get('/api/todo', (req, res) => {
  todo.getTodos({}, (err, docs) => {
    console.log(err);
    console.log(docs);
    res.send(docs);
  })
});

app.post('/api/todo', (req, res) => {
  const data = req.body;
  console.log(data);
  addTodo.addTodo(data, (err, dbrs) => {
    if (err) {
      res.status(500).send('Unable to insert todo');
      return;
    }
    res.status(201).send(dbrs.insertedIds[0] || '');
  });
});


// Returns a user with the given id or undefined if no user exists with the given id
const getUserById = id => {
  const idInt = parseInt(id);
  if (isNaN(id) || idInt < 0) {
    return undefined;
  }
  const user = users.filter(user => user.id === id);
  if (user.length === 0) {
    return undefined;
  }
  return user[0];
};

// Returns a company with the given id or undefined if no company exists with the given id
const getCompanyById = id => {
  const idInt = parseInt(id);
  if (isNaN(id) || idInt < 0) {
    return undefined;
  }
  const company = companies.filter(company => company.id === id);
  if (company.length === 0) {
    return undefined;
  }
  return company[0];
};

/*
 * Returns a list of all companies
 * GET /api/companies
 */
app.get('/api/companies', (req, res) => {
    res.type('application/json');
    res.statusCode = 200;
    const result = companies;
    return res.send(result);
});

/*
 * Adds a new company to the list of companies.
 * Required parameters: String name, integer punchCount
 * Example:
 * POST /api/companies
 * Request body:
 * {
 *    "name": "The umbrella corporation",
 *    "punchCount": 5
 * }
 */
app.post('/api/companies', (req, res) => {
  const company = req.body;
  if (typeof company.name !== 'string' ||
      company.name === '' ||
      typeof company.punchCount !== 'number' ||
      company.punchCount === ''
  ) {
    res.statusCode = 400;
    return res.send('To add a new company the following properties need to be specified: name, punchCount');
  }
  const punchCount = parseInt(company.punchCount, 10);
  if (isNaN(punchCount) || punchCount < 1) {
    res.statusCode = 404;
    return res.send('The company punchCount must be a positive number');
  }
  const newCompany = { punchCount, name: company.name, id: companyId++ };
  companies.push(newCompany);
  res.statusCode = 201;
  res.location(serverUrl + '/api/companies/' + newCompany.id);
  return res.json(newCompany);
});

/*
 * Returns a company with the requested id
 * Example:
 * GET /api/companies/1
 * Returns the company with id 1
 */
app.get('/api/companies/:id', (req, res) => {
  const id = parseInt(req.params.id);
  if (isNaN(id) || id < 0) {
    res.statusCode = 404;
    return res.send('id must be a positive number');
  }
  const company = companies.filter(company => company.id === id);
  if (company.length === 0) {
    res.statusCode = 404;
    return res.send();
  }
  res.statuscode = 200;
  const returnComp = company[0];
  return res.json(company[0]);
});

/*
 * Returns a list of all users
 * GET /api/users
 */
app.get('/api/users', (req, res) => {
    res.type('application/json');
    res.statusCode = 200;
    const result = users;
    return res.send(result);
});

/*
 * Adds a new user to the list of users
 * Required parameters: String name, String email
 * Example:
 * POST /api/users
 * Request body:
 * {
 *    "name": "John Doe",
 *    "email": "johnd@example.com"
 * }
 */
app.post('/api/users', (req, res) => {
  const user = req.body;
  if (typeof user.name !== 'string' ||
      user.name === '' ||
      typeof user.email !== 'string' ||
      user.email === ''
  ) {
    res.statusCode = 400;
    res.send('To add a new user the following properties need to be specified: name, email');
  }
  const newUser = { name: user.name, email: user.email, id: userId++, punches: [] };
  users.push(newUser);
  res.statusCode = 201;
  res.location(serverUrl + '/api/users/' + newUser.id);
  return res.json(newUser);
});

/*
 * Returns a list of all punches for a given user or a list of punches of
 * a user for a given company by id.
 * Examples:
 * GET /api/users/1/punches
 * Returns a list of all punches of user with id 1
 * GET /api/users/1/punches?company=2
 * Returns a list of all punches of company with id 2 from user with id 1
 */
app.get('/api/users/:id/punches', (req, res) => {
  const query = url.parse(req.url,true).query;
  const userId = parseInt(req.params.id);
  if (isNaN(userId) || userId < 0) {
    res.statusCode = 404;
    return res.send('The user id must be a positive number');
  }
  const user = getUserById(userId);
  if (user === undefined) {
    res.statusCode = 404;
    return res.send('no user was found with the given id');
  }
  if (query.company !== undefined) {
    const companyId = parseInt(query.company);
    if (isNaN(companyId) || companyId < 0) {
      res.statusCode = 404;
      return res.send('The company id must be a positive number');
    }
    const companyPunches = user.punches.filter(punch => punch.id === companyId);
    res.statusCode = 200;
    return res.send(companyPunches);
  }
  res.statusCode = 200;
  return res.send(user.punches);
});

/*
 * Adds a punch to the punchlist of a user by company id
 * Example:
 * POST /api/users/1/punches
 * Request body:
 * {
 *    "id": 2
 * }
 * Adds a punch for company with id 2 to the list of punches of user with id 1
 */
app.post('/api/users/:id/punches', (req, res) => {
  const userId = parseInt(req.params.id);
  if (isNaN(userId) || userId < 0) {
    res.statusCode = 404;
    return res.send('The user id must be a positive number');
  }
  const user = getUserById(userId);
  if (user === undefined) {
    res.statusCode = 404;
    return res.send('no user was found with the given id');
  }
  if (req.body.id === undefined) {
    res.statusCode = 400;
    return res.send('required properties: id');
  }
  const companyId = parseInt(req.body.id);
  if (getCompanyById(companyId) === undefined) {
    res.statusCode = 400;
    return res.send('No company was found with the given id')
  }
  const punch = { id: companyId, created: new Date().toLocaleString() };
  user.punches.push(punch);
  res.statusCode = 201;
  res.location(serverUrl + '/api/users/' + userId + '/punches' + '?company=' + companyId);
  return res.send(user.punches);
});

app.listen(process.env.PORT || serverPort);
