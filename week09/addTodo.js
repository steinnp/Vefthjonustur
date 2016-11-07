'use strict';
const MongoClient = require('mongodb').MongoClient;
const url = 'mongodb://localhost:27017/punch';

const getTodos = (query, cb) => {
  MongoClient.connect(url, (err, db) => {
    const collection = db.collection('todo');
    collection.find(query).toArray((err, docs) => {
      cb(null, docs);
      db.close();
    });
  });

}

const addTodo = (todo, cb) => {
  MongoClient.connect(url, (err, db) => {
    if (err) {
      cb(err);
      db.close();
      return;
    }

    const collection = db.collection('todo');
    collection.insert(todo, function(ierr, res) {
      if(ierr) {
        cb(ierr);
        db.close();
        return;
      }
      cb(null, res);
    });
  });
};

module.exports = {
  addTodo: addTodo,
  getTodos: getTodos,
}
