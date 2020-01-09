var connect = require('connect');
var serveStatic = require('serve-static');
var opn = require('opn');

connect().use(serveStatic(__dirname)).listen(14568, function(){
    console.log('Server running on http://localhost:14568...');
    opn('http://localhost:14568');
});