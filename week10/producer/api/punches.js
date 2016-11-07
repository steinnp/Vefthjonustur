'use strict';
const express = require('express');
const router = express.Router();
const entities = require('../entities');
const uuid = require('node-uuid');
const ObjectId = require('mongodb').ObjectID;
const amqp = require('amqplib/callback_api');

// constants
const ADMIN_TOKEN = require('../constants').ADMIN_TOKEN;
const SERVER_URL = require('../constants').SERVER_URL;
const MODULE_URL = `${SERVER_URL}/api/my/punches`;

/*
 * /api/my/punches
 */

/*
 * takes in a user, punch and company object as well as the unused punch count
 * and posts the id and name of the user,
 * the name, id and punchcount of the company, the date of the punch and
 * how many unused punches the user has to rabbitmq:
 */
const postPunch = (user, punch, company, punchCount) => {
  const punchString = JSON.stringify({
      name: user.name,
      userId: user['_id'],
      companyId: punch.companyId,
      companyName: company.name,
      companyPunchCount: company.punchCount,
      unusedPunches: punchCount,
      created: Date.now(),
  });
  amqp.connect('amqp://localhost', function(err, conn) {
    conn.createChannel(function(err, ch) {
      const ex = 'topic_logs';
      const key = 'user.punch';

      ch.assertExchange(ex, 'topic', {durable: false});
      ch.publish(ex, key, new Buffer(punchString));
      console.log(" [x] Sent %s:'%s'", key, punchString);
    });
  });
 }

/*
 * takes in a user, punch and company object as well as the unused punch count
 * and posts the id and name of the user,
 * the name, id and punchcount of the company, the date of the punch and
 * how many unused punches the user has to rabbitmq:
 */
 const postDiscount = (user, punch, company) => {
  const discountString = JSON.stringify({
      name: user.name,
      userId: user['_id'],
      companyId: punch.companyId,
      companyName: company.name,
      companyPunchCount: company.punchCount,
      created: Date.now(),
  });
  amqp.connect('amqp://localhost', function(err, conn) {
    conn.createChannel(function(err, ch) {
      const ex = 'topic_logs';
      const key = 'user.discount';

      ch.assertExchange(ex, 'topic', {durable: false});
      ch.publish(ex, key, new Buffer(discountString));
      console.log(" [x] Sent %s:'%s'", key, discountString);
    });
  });
 }



/*
 * Adds a punch to the punchlist of the current user by company id
 * Example:
 * POST /api/my/punches
 * Request body:
 * {
 *    "company_id": 2
 * }
 * Adds a punch for company with id 2 to the list of punches of user with id 1
 */
router.post('/', (req, res) => {

  if (req.body['company_id'] === undefined) {
    res.statusCode = 412;
    res.send('company_id not defined');
    return;
  }

  // the user object corresponding to the given token will be assigned to this
  let user;

  const userPunchesPromise = entities.Users.find({token: req.headers.authorization})
  .then(userResult => {
    if (userResult.length === 0) {
      res.statusCode = 401;
      res.send();
      return;
    }
    user = userResult[0];
    return user['_id'];
  })
  // punches depend on the user id so we cannot query mongo for punches in paralell
  .then(result => {
    return entities.Punches.find(
      {
        userId: result,
        companyId: req.body['company_id'],
        used: false,
      });
  });

  // company query
  const companyPromise = entities.Companies.find({_id: ObjectId(req.body['company_id'])})
  .then(companyResult => {
    if (companyResult.length === 0) {
      res.statusCode = 404;
      res.send();
      return;
    }
    return companyResult;
  });

  // wait for both queries to finish
  return Promise.all([userPunchesPromise, companyPromise])
  .then(values => {
    const punches = values[0];
    const company = values[1][0];
    const newPunch = new entities.Punches({
      companyId: company['_id'],
      userId: user['_id'],
    });
    if (punches.length !== (company.punchCount - 1)) {
      newPunch.save();
      res.statusCode = 201;
      postPunch(user, newPunch, company, punches.length);
      res.send(newPunch['_id']);
    } else {
      newPunch.used = true;
      punches.push(newPunch);
      // update the used status of every unused punch
      Promise.all(
        punches.map(punch => {
          punch.used = true;
          punch.save();
        })
      )
      .then(() => {
        postDiscount(user, newPunch, company);
        res.statusCode = 201;
        res.send({ discount: true });
      });

    }
  }).catch(err => {
    console.log(err);
    res.statusCode = 500;
    res.send('The server could not process your request');
    return;
  });
});



module.exports = router;
