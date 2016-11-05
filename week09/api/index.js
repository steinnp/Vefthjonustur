'use strict';
const express = require('express');
const router = express.Router();

router.use('/companies', require('./companies'));
router.use('/users', require('./users'));
router.use('/my/punches', require('./punches'));

module.exports = router;
