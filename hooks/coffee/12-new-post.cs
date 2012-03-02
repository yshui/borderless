new_post = ->
	return (req, res, next) ->
		proper_encode = (str) ->
			return str.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;")
		validate_form = (body) ->
			return body.title.match(/^\s*$/) and body.content.match(/^\s*$/)
		libs = req._my_libs
		data = req._my_data
		if req.method != 'POST'
			next()
		else
			#if not validate_form req.body
			#	data.res.badreq.push "You must fill all fields"
			#	next()
			#	return
			#if not req.session.permission or not req.session.permission.write
			#	data.res.forbidden.push "Permission denied"
			#	next()
			#	return
			html = libs.md proper_encode(req.body.content), true
			date = new Date()
			po = {
				title: proper_encode(req.body.title),
				author: req.session.user_name,
				tags: proper_encode(req.body.tags).split(/\s+/),
				category: proper_encode(req.body.category),
				post_id: libs.rb.randomBytes(20).toHex(),
				create_date: date.toUTCString(),
				modify_date: date.toUTCString(),
				snippet: html.split('<!-- more -->')[0]
			}
			libs.fs.writeFile("#{libs.doc_root}/#{po.post_id}.html", html, (err) ->
				if err
					data.res.error.push "write error #{err.toString()}"
					next()
				else
					data.res.ok.push(po)
					data.new_post_data = po
					next()
			)

module.exports.init = (app) -> app.use '/ajax/post', new_post()

