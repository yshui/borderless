_get = (req, res, next, type) ->
	libs = req._my_libs
	if(!req._my_data)
		req._my_data = {}
	if(req.method != 'GET')
		next()
	else
		urlb = libs.url.parse req.url, true
		if(urlb.query.type != "#{type}")
			return next()
		libs.redis.keys("#{type}:*", (err, reps) ->
			tags = []
			tags.push(rep.replace("#{type}:", "")) for rep in reps

			tmp = {}
			tmp[type.toString()] = tags
			req._my_data.res.ok.push tmp
		)
get_categories = ->
	return (req, res, next) ->
		_get(req, res, next, "category")
get_tags = ->
	return (req, res, next) ->
		_get(req, res, next, "tag")

module.exports.init = (app) ->
	app.use '/ajax/get', get_tags()
	app.use '/ajax/get', get_categories()
