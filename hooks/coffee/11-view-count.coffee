add_view_count = ->
	return (req, res, next) ->
		id = req.body.post_id
		if(!id)
			return next()
		libs.redis.incr("post_vc:#{id}", 1, (err, re) ->
			if(err)
				data.res.error.push "Redis error: #{err.toString()}"
			else
				data.res.ok.push {'view_count': re}
			next()
		)
get_view_count = ->
	return (req, res, next) ->
		libs = req._my_libs
		if(!req._my_data.post_data or req._my_data.post_data.length <= 0)
			return next()
		m = libs.redis.multi()
		m.get("post_vc:#{pd.post_id}") for pd in req._my_data.post_data
		m.exec((err, reps) ->
			if(err)
				data.res.error.push "Redis error: #{err.toString()}"
				return next()
			else
				for i in [0..reps.length-1]
					req._my_data.post_data[i].view_count = if reps[i] then reps[i] else 0
				return next()
		)
module.exports.init = (app) ->
	app.use '/ajax/view', add_view_count()
	app.use get_view_count()
