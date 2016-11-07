'use strict';
const express = require('express');
const url = require('url') ;
const bodyParser = require('body-parser');
const app = express();
const MongoClient = require('mongodb').MongoClient;
const mongoose = require('mongoose');
const SERVER_PORT = require('./constants').SERVER_PORT;
const DATABASE_URL = require('./constants').DATABASE_URL;


const dbUrl = 'mongodb://localhost:27017/app';

// importing other project modules

const api = require('./api');

app.use(bodyParser.json());
app.use('/api', api);

mongoose.connect(DATABASE_URL);
mongoose.connection.once('open', (res, err) => {
  console.log('Connected to database');
  app.listen(SERVER_PORT, () => {
    console.log(`Web server started on port: ${SERVER_PORT}`);
  });
});
