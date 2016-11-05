'use strict';
const mongoose = require('mongoose');

const UsersSchema = new mongoose.Schema({
  name: {
    type: String,
    required: [true, 'name is required'],
  },
  token: {
      type: String,
      required: [true, 'token is required'],
  },
  gender: {
    type: String,
    required: [true, 'gender is required'],
    validate: {
      validator: value => {
        if (value === 'm' || value === 'f' || value === 'o') {
          return true;
        }
        return false;
      },
      message: 'gender must be m, f or o',
    },
  },
});

const Users = mongoose.model('Users', UsersSchema);

module.exports = Users;
