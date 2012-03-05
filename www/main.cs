loaded_page = -1
doc_root = ""
post_list = []
postblock_template = new EJS({url: "/postblock.ejs"})
build_post_list = (arr) ->
	$("#post_list").empty()
	for i in arr
		if not i.tags
			i.tags = []
		if not i.tags.length
			i.tags = [i.tags]
		blockhtml = postblock_template.render(i)
		$("#post_list").append blockhtml
load_post_list = (start, end, category, tag)->
	url = "#{doc_root}/ajax/get?start=#{start}&end=#{end}"
	if(tag and tag isnt "")
		url += "&tag=#{tag}"
	else if (category and category isnt "")
		url += "&category=#{category}"
	$.ajax({
		url: url,
		dataType: "json",
		success: (obj) ->
			for i in obj
				if(i.post_data)
					build_post_list(i.post_data)
	})
change_mf_tag = ->
	return
change_mf_category = ->
	tmp = location.hash
	tmp = tmp.split "/"
	category = tmp[0].replace "#category-", ""
	start = 0
	end = start+24
	if(tmp[1])
		start = tmp[1]*25-25
		end = start+24
	load_post_list(start, end, category)
	return
expand_post = ->
	return
register_event_handler = ->
	$(window).hashchange( ->
		if location.hash.match /^#category/
			change_mf_category()
		else if location.hash.match /^#tag/
			change_mf_tag()
		else if location.hash.match /^#post/
			expand_post()
		else
			load_post_list(0, 24)
		return
	)
	return
$(document).ready( ->
	register_event_handler()
	load_post_list(0, 24)
)
