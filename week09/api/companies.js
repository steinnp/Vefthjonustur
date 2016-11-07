'use strict';
const express = require('express');
const router = express.Router();
const entities = require('../entities');
const ObjectId = require('mongodb').ObjectID;

// constants
const ADMIN_TOKEN = require('../constants').ADMIN_TOKEN;
const SERVER_URL = require('../constants').SERVER_URL;
const MODULE_URL = `${SERVER_URL}/api/companies`;

/*
 * /api/companies
 */

/*
 * Returns a list of all companies
 * GET /api/companies
 */
router.get('/', (req, res) => {
  entities.Companies.find()
  .then(companies => {
    res.statusCode = 200;
    res.send(companies);
    return;
  })
  .catch(err => {
    res.statusCode = 500;
    res.send('The server could not process your request');
    return;
  });
});

/*
 * Returns a company with the requested id
 * Example:
 * GET /api/companies/1
 * Returns the company with id 1
 */
router.get('/:id', (req, res) => {
  entities.Companies.find({ _id: ObjectId(req.params.id) })
  .then(company => {
    if (company.length === 0) {
      res.statusCode = 404;
      res.send();
      return;
    }
    res.statusCode = 200;
    res.send(company);
    return;
  })
  .catch(err => {
    res.statusCode = 500;
    res.send('The server could not process your request');
    return;
  });
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
router.post('/', (req, res) => {
  if (req.headers.authorization !== ADMIN_TOKEN) {
    res.statusCode = 401;
    res.send();
    return;
  }
  const company = new entities.Companies(req.body);
  company.save()
  .then(company => {
    res.statusCode = 201;
    res.location(`${MODULE_URL}/${company['_id']}`);
    res.send(company);
    return;
  })
  .catch(err => {
    if (err.name === 'ValidationError') {
      res.statusCode = 412;
      res.send(err.message);
      return;
    } else {
      res.statusCode = 500;
      res.send();
      return;
    }
  });
});


module.exports = router;
