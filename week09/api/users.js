'use strict';
const express = require('express');
const router = express.Router();
const entities = require('../entities');
const uuid = require('node-uuid');
const ObjectId = require('mongodb').ObjectID;

// constants
const ADMIN_TOKEN = require('../constants').ADMIN_TOKEN;
const SERVER_URL = require('../constants').SERVER_URL;
const MODULE_URL = `${SERVER_URL}/api/users`;

/*
 * /api/users
 */


/*
 * Adds a new user to the list of users
 * Required parameters: String name, String gender (m, f or o)
 * Example:
 * POST /api/users
 * Request body:
 * {
 *    "name": "John Doe",
 *    "gender": "m"
 * }
 */
router.post('/', (req, res) => {
  if (req.headers.authorization !== ADMIN_TOKEN) {
    res.statusCode = 401;
    res.send();
    return;
  }
  const user = new entities.Users(req.body);
  const token = uuid.v1();
  user.token = token;
  user.save()
  .then(user => {
    res.statusCode = 201;
    res.send({ token })
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

/*
 * Returns a list of all users
 * GET /api/users
 */
router.get('/', (req, res) => {
  entities.Users.find()
  .then(users => {
    const usersNoToken = users.map(user => {
      return {
        name: user.name,
        gender: user.gender,
      };
    });
    res.statusCode = 200;
    res.send(usersNoToken);
    return;
  })
  .catch(err => {
    res.statusCode = 500;
    res.send('The server could not process your request');
    return;
  });
});

module.exports = router;
