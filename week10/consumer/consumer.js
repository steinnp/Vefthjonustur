const amqp = require('amqplib/callback_api');

amqp.connect('amqp://localhost', function(err, conn) {
  conn.createChannel(function(err, ch) {
    var ex = 'topic_logs';

    //ch.assertExchange(ex, 'topic', {durable: false});

    ch.assertQueue('topic', {exclusive: true}, function(err, q) {
      console.log(' [*] Waiting for logs. To exit press CTRL+C');

        ch.bindQueue('topic', ex, 'user.created');
        ch.bindQueue('topic', ex, 'user.punch');
        ch.bindQueue('topic', ex, 'user.discount');

      ch.consume(q.queue, function(msg) {
        if (msg.fields.routingKey === 'user.created') {
          console.log("User was added");
        } else if (msg.fields.routingKey === 'user.punch') {
          console.log("User got a punch");
        } else if (msg.fields.routingKey === 'user.discount') {
          console.log("User got a discount");
        }
        //console.log(" [x] %s:'%s'", msg.fields.routingKey, msg.content.toString());
      }, {noAck: true});
    });
  });
});
