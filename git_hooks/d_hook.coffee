filename = process.argv[2]
doc_root = "."
fs = require 'fs'
filecontent = fs.readFileSync(filename, 'utf8')
metadata = JSON.parse(filecontent.split('\n')[0])
metadata.post_id = filename
try
	fs.unlinkSync("#{doc_root}/#{metadata.post_id}.html")
catch err
	console.log(err)
client = require('redis').createClient()
client.exists("post:#{metadata.post_id}", (err, rep) ->
	m = client.multi()
	if rep != 0
		m.del("post:#{metadata.post_id}")
		m.lrem("blog_post_list", 0, metadata.post_id)
		if(metadata.tags)
			m.lrem("tag:#{tag}", 0, metadata.post_id) for tag in metadata.tags
		if(metadata.category)
			m.lrem("category:#{metadata.category}", 0, metadata.post_id)

	m.exec((err, rep) ->
		client.quit()
		if err
			return 1
		return 0
	)
)
