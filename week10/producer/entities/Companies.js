'use strict';
const mongoose = require('mongoose');

const CompaniesSchema = new mongoose.Schema({
  name: {
    type: String,
    required: [true, 'name is required'],
  },
  punchCount: {
    type: Number,
    default: 10,
  }
});

const Companies = mongoose.model('Companies', CompaniesSchema);

module.exports = Companies;
