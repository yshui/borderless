get_feed = (req, res, next) ->
	libs = req._my_libs
	feed = new libs.rss({
		title: 'xbqo\'s blog',
		description: 'xbqo\'s blog',
		feed_url: 'http://blog.rorvn.info/rss.xml',
		site_url: 'http://blog.rorvn.info',
		author: 'xbqo'
	})
	libs.redis.lrange('blog_post_list', 0, 100, (err, reps) ->
		if(err)
			res.err().text("Error")
			return
		m = libs.redis.multi()
		m.get("post:#{rep}") for rep in reps
		m.exec((e, r) ->
			if(e)
				res.err().text("Error")
				return
			for re in r
				md = JSON.parse(re)
				md.title = "(NO NAME)" if not md.title
				md.snippet = "(UNKNOWN)" if not md.snippet
				md.author = "xbqo" if not md.author
				feed.item({
					title:  md.title,
					description: md.snippet,
					url: "http://blog.rorvn.info/p/#{md.post_id}.html",
					author: md.author,
					date: md.modify_time
				})
			res.xml(feed.xml())
		)
	)
module.exports.init = (app) ->
	app.use '/ajax/feed', get_feed
