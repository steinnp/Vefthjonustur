const express = require('express');
const router = express.Router();
const entities = require('./entities');
const uuid = require('node-uuid');


router.get('/', (req, res) => {
  entities.Users.find().then(res => {
    res.send(res);
    return;
  }).catch(err => {
    res.send(err);
    return;
  });
});

router.post('/', (req, res) => {
  const data = req.body;
  connect()
  .then(db => {
    const collection = db.collection('todo');
    collection.insertOne(data)
    .then(result => {
      res.send('success');
      db.close();
    }).catch(ierr => {
      //console.log(ierr);
      res.send('error');
      db.close();
    });
  })
  .catch(err => {
    //  console.log(err);
    res.send(err);
    db.close();
  });
});

module.exports = router;
