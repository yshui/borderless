_data_init = (req) ->
	req._my_data = {res: {error: [], badreq: [], forbidden: [], ok: []}}
data_init = ->
	return (req, res, next) ->
		_data_init(req)
		next()
init = (app) ->
	app.use(data_init())
module.exports.init = init
