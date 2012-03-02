do_response = ->
	return (req, res, next) ->
		if req._my_data.res.error.length > 0
			res.error().json req._my_data.res.error
		else if req._my_data.res.badreq.length > 0
			res.badRequest().json req._my_data.res.badreq
		else if req._my_data.res.forbidden.length > 0
			res.forbidden().json req._my_data.res.forbidden
		else if req._my_data.res.ok.length > 0
			res.ok().json req._my_data.res.ok
		else
			next()

module.exports.init = (app) -> app.use do_response()

