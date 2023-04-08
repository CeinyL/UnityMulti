const userData = require('./userData');

let userList = [];

let message = {
    Type: '',
    Content: ''
};


const getUserData = async (Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = createUserId();
    if(user.username == '') user.username = createUserId();

    let content = JSON.stringify(user);

    message = 
    {
        Type: 'userData',
        Content: content
    }

    return 0;
};

const HandleValidation =  async (socket,msg) => 
{
  let jsonmsg = JSON.parse(msg)
  let id = await userData.createUserId();
  let usr= "";
  /// ErrorCode 
  /// 0 - succes
  /// 1 - wrong username
  /// 2 - later
  let isErrorCode = 1;
  let isValid = false;
  if(jsonmsg.Content.username==""||jsonmsg.Content.username==null)usr=id;
  else usr=jsonmsg.Content.username;
  
  if(jsonmsg.Content.username.localeCompare('Betek')==0)// case sensitive with no accents
  {
    isValid = true
    isErrorCode = 0;
  } 
  //-1 - username has wrong casing 0 - correct nad 1 - wrong // pewnie potem haslo i zapytanie do abzy danych
  message = 
  {
    Type : messageTypes.RESVALIDATION,
    Content : 
    {
      UserID : id,
      Username : usr,
      Validated : isValid,
      ErrorCode : isErrorCode
    }

  }
  console.log(message);
  socket.send(JSON.stringify(message));
}
const HandlePing = async (socket,msg) => {
    message = 
    {
      Type: messageTypes.PONG,
      Timestamp: Date.now() // CHANGE LATER
    };
    socket.send(JSON.stringify(message));
  };
  

module.exports = {
    startupMessage,
    HandleValidation,
    HandlePing
  };