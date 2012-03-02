var hooks    = [];
var fs       = require('fs'),
    redis    = require('redis'),
    connect  = require('connect'),
    quip     = require('quip'),
    rb       = require('rbytes'),
    md       = require('node-markdown').Markdown,
    http     = require('http'),
    url      = require('url');

var client = redis.createClient();
var doc_root = '/var/shui/www/';
merge = function(a, b){
	if (a && b) {
		for (var key in b) {
			a[key] = b[key];
		}
	}
	return a;
};

var libs = {
	redis    : client,
	rb       : rb,
	md       : md,
	fs       : fs,
	doc_root : doc_root,
	merge    : merge,
	url      : url
}
client.on('error', function (err) {
	console.log("Redis error: " + err);
});
server_start = function (app) {
	app.listen(12788);
}
add_libs = function (){
	return function(req, res, next){
		req._my_libs = libs;
		next();
	}
}
null_end = function (){
	return function(req, res, next){
		res.end();
		return;
	}
}
run_hooks = function (err, files) {
	var app = connect()
		.use(add_libs())
		.use(connect.bodyParser())
		.use(quip())
		.use(connect.cookieParser())
		.use(connect.session({secret: "thisisatest"}));

	files = files.sort();
	for (i in files){
		if(files[i].match(/\.js$/))
			require('./hooks/'+files[i]).init(app);
	}
	server_start(app);
}
fs.readdir("hooks", run_hooks);
