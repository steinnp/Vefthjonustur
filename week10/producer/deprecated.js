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

const Users = mongoose.model('Users', UsersSchema);
const Companies = mongoose.model('Companies', CompaniesSchema);
const Punches = mongoose.model('Punches', PunchSchema);


const entities = {
  Users,
  Companies,
  Punches,
}

module.exports = entities;
