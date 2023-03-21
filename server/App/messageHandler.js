const userData = require('./userData');

let startupMessage = {
    Type: 'requestUserData',
    Content: 'null'
};

const HandleMessage = (data) => {
    console.log("lol");
};

module.exports = {
    startupMessage,
    HandleMessage
  };