const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

const createUserId = () => {
    let userId = '';
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      userId += chars[randomIndex];
    }
    return userId;
};

const createUsername = () => {
    let username = '';
    for (let i = 0; i < 8; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      username += chars[randomIndex];
    }
    return username;
};

let user = {
    userId: createUserId(),
    username: '',
};

module.exports = {
    user
  };