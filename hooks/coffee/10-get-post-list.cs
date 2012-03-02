get_posts = ->
	return (req, res, next) ->
		libs = req._my_libs
		if(!req._my_data)
			req._my_data = {}
		if(req.method != 'GET')
			next()
		else
			urlb = libs.url.parse req.url, true
			if(urlb.query.type and urlb.query.type != "post")
				return next()
			start = 0
			end = 49
			if(parseInt(urlb.query.start) >= 0)
				start = parseInt(urlb.query.start)
				if(!urlb.query.end || parseInt(urlb.query.end) <= start)
					end = start+49
				else
					end = parseInt(urlb.query.end)
			_get_post_data = (err, reps) ->
				m = libs.redis.multi()
				m.get("post:#{rep}") for rep in reps
				m.exec((err, reps) ->
					if(err)
						err_msg = "Redis error: #{err.toString()}"
						req._my_data.res.error.push err_msg
						next()
					else
						req._my_data.post_data = []
						for rep in reps
							tmp = JSON.parse(rep)
							if(tmp)
								req._my_data.post_data.push(tmp)
						req._my_data.res.ok.push {post_data : req._my_data.post_data}
						next()
					
				)

			if(urlb.query.tag)
				libs.redis.lrange("tag:#{urlb.query.tag}", start, end, _get_post_data)
			else if (urlb.query.category)
				libs.redis.lrange("category:#{urlb.query.category}", start, end, _get_post_data)
			else
				libs.redis.lrange('blog_post_list', start, end, _get_post_data)

module.exports.init = (app) ->
	app.use '/ajax/get', get_posts()
