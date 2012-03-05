filename = process.argv[2]
doc_root = "."
fs = require 'fs'
filecontent = fs.readFileSync(filename, 'utf8')
metadata = JSON.parse(filecontent.split('\n')[0])
metadata.post_id = filename
metadata.create_time = new Date
metadata.modify_time = new Date
if !metadata.tags
	metadata.tags = []
if !metadata.tags.length
	metadata.tags = [metadata.tags]
filecontent = filecontent.replace /[^\n]*\n/, ""
md = require('node-markdown').Markdown
html = md filecontent, true
if !metadata.snippet
	metadata.snippet = html.split('<!-- more -->')[0]
fs.writeFileSync("#{doc_root}/#{metadata.post_id}.html", html, 'utf8')
client = require('redis').createClient()
client.get("post:#{metadata.post_id}", (err, rep) ->
	m = client.multi()
	if !rep
		m.lpush("blog_post_list", metadata.post_id)
		if(metadata.tags)
			m.lpush("tag:#{tag}", metadata.post_id) for tag in metadata.tags
		if(metadata.category)
			m.lpush("category:#{metadata.category}", metadata.post_id)
	else
		metadata.create_time = JSON.parse(rep).create_time
	m.set("post:#{metadata.post_id}", JSON.stringify(metadata))
	m.exec((err, rep) ->
		client.quit()
		if err
			return 1
		return 0
	)
)
