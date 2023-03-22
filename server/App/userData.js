const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

const createUserId = async () => {
    let userId = '';
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      userId += chars[randomIndex];
    }
    return userId;
};

const createUsername = async () => {
    let username = '';
    for (let i = 0; i < 8; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      username += chars[randomIndex];
    }
    return username;
};

let user = {
    userId: '',
    username: '',
};

module.exports = {
    user,
    createUserId,
    createUsername
  };