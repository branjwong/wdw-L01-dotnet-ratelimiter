const express = require('express');
const app = express();
const port = 3000;

app.get('/', function (req, res) {
  res.sendFile('index.html', {
    root: __dirname
  });
});

app.get('/index.css', function (req, res) {
  res.sendFile('index.css', {
    root: __dirname
  });
});

app.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
});
