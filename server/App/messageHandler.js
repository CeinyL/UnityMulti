const userData = require('./userData');

let userList = [];

let message = {
    Type: '',
    Content: ''
};

const HandleMessage = async (data) => {
    message = JSON.parse(data);
    switch(message.Type){
        case "getUserData":
            await getUserData(message.Content);
            break;
        default:
            break;
    }
};

const getUserData = async (Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = createUserId();
    if(user.username == '') user.username = createUserId();

    let content = JSON.stringify(user);

    message = {
        Type: 'userData',
        Content: content
    }

    return 0;
};

module.exports = {
    startupMessage,
    HandleMessage
  };