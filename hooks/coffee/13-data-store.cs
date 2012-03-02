data_store = ->
	return (req, res, next) ->
		libs = req._my_libs
		fail = (pdata) ->
			req._my_data.res.badreq.push("Invalid arguments")
			return next()
		succ = (pdata) ->
			libs.redis.set("post:#{pdata.post_id}", JSON.stringify(pdata), (err, rep) ->
				if err
					req._my_data.res.error.push("Redis error: #{err.toString()}")
					return next()
				else
					m = libs.redis.multi()
					m.lpush("tag:#{tag}", pdata.post_id) for tag in pdata.tags
					m.lpush("category:#{pdata.category}", pdata.post_id)
					m.lpush("blog_post_list", pdata.post_id)
					m.exec((err, reps) ->
						if err
							req._my_data.res.error.push("Redis error: #{err.toString()}")
						return next()
					)
			)
		validate_post_data = (pdata) ->
			libs.redis.exists("user:#{pdata.author}", (err, rep) ->
				#if not rep
				#	return fail(pdata)
				#else
					return succ(pdata)
			)
		if(req._my_data.new_post_data)
			validate_post_data(req._my_data.new_post_data)
		else
			return next()
module.exports.init = (app) -> app.use data_store()
