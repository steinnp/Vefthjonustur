'use strict';
const mongoose = require('mongoose');

const PunchSchema = new mongoose.Schema({
  companyId: {
    type: String,
    required: [true, 'companyId is required'],
  },
  userId: {
    type: String,
    required: [true, 'userId is required'],
  },
  created: {
    type: Date,
    default: new Date(),
  },
  used: {
    type: Boolean,
    default: false,
  },
});

const Punches = mongoose.model('Punches', PunchSchema);

module.exports = Punches;
