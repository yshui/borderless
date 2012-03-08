loaded_page = -1
doc_root = ""
post_list = []
postblock_template = new EJS({url: "/postblock.ejs"})
page_para = ->
	return {category: "", tag: "", page:0, post:""}
page_parameters = {}
parse_fragment_id = (frag) ->
	frag = frag.replace /^#/, ""
	frag = frag.replace /^\//, ""
	para = frag.split '/'
	npara = new page_para()
	for i in para
		for j,v of npara
			tmp = new RegExp("^#{j}-")
			if tmp.test(i)
				npara[j] = i.replace tmp, ""
	return npara
build_fragment_id = (para) ->
	frag = ""
	for i,v of para
		if v != "" and (parseInt(v) != 0 and parseInt(v) != NaN or i != "page")
			frag += "#{i}-#{v}/"
	return frag
register_post_block_event_handler = ->
	$(".post_tag_button").click( (e) ->
		if(e.which == 1)
			newtag = $(this).attr("id").replace /^ptb_/, ""
			newfrag = build_fragment_id($.extend({},page_parameters, {tag:newtag}))
			location.hash = newfrag
			e.preventDefault()
	)
	$(".post_category_button").click( (e) ->
		if(e.which == 1)
			newc = $(this).attr("id").replace /^pcb_/, ""
			newfrag = build_fragment_id($.extend({},page_parameters, {category:newc}))
			location.hash = newfrag
			e.preventDefault()
	)
build_post_list = (arr) ->
	$("#post_list").empty()
	for i in arr
		if not i.tags
			i.tags = []
		if not i.tags.length
			i.tags = [i.tags]
		blockhtml = postblock_template.render(i)
		$("#post_list").append blockhtml
	register_post_block_event_handler()
append_prop = ->
	for i in $(".post_content")
		$(i).css("display", "none")
	for i in $(".post_snippet")
		$(i).css("height", $(i).height())
update_category_tag = (para, callback)->
	$("#main_frame").addClass("loading")
	start = para.page*25
	end = start+24
	url = "#{doc_root}/ajax/get?start=#{start}&end=#{end}&category=#{para.category}&tag=#{para.tag}"
	$.ajax({
		url: url,
		dataType: "json",
		success: (obj) ->
			for i in obj
				if(i.post_data)
					build_post_list(i.post_data)
					break
			$("#main_frame").removeClass("loading")
			append_prop()
			if callback
				callback()
	})
collapse_post = (co) ->
	post = $(co).attr("id").replace /^pco_/, ""
	sn = $("#ps_#{post}")[0]
	th = $(sn).height()
	tch = $(co).height()
	$(sn).css("position", "absolute")
	$(sn).css("opacity", 0)
	$(co).css("opacity", 0)
	$(co).css("height", "#{th}px")
	$(sn).css("display", "block")
	setTimeout( ->
		$(sn).css("opacity", "")
		$(sn).css("position", "")
		$(co).css("display", "none")
		$(co).css("height", "")
		$(co).css("height", "#{$(co).height()}px")
		$(co).css("opacity", "")
	, 1000)
expand_post = (post) ->
	for i in $(".post_content")
		if $(i).css("display") != "none"
			collapse_post i
	if not post or post == ""
		return
	$.ajax({
		url: "/p/#{post}.html",
		success: (html) ->
			$("#pco_#{post}").html(html)
			co = $("#pco_#{post}")
			sn = $("#ps_#{post}")
			co.css("position", "absolute")
			co.css("opacity", 0)
			co.css("height", co.height())
			sn.css("opacity", 0)
			sn.css("height", "#{co.height()}px")
			co.css("display", "block")
			setTimeout( ->
				co.css("opacity", "")
				co.css("position", "")
				sn.css("display", "none")
				sn.css("height", "")
				sn.css("height", "#{sn.height()}px")
				co.css("opacity", "")
			, 1000)
	})
	return
update_page = (para) ->
	if para.category != page_parameters.category or para.tag != page_parameters.tag or para.page != page_parameters.page
		update_category_tag(para, ->
			expand_post(para.post)
		)
	expand_post(para.post)
update_topbar = ->
	$(".category_indicator").html(if page_parameters.category then page_parameters.category else "---")
	$(".tag_indicator").html(if page_parameters.tag then page_parameters.tag else "---")
register_event_handler = ->
	$(window).hashchange( ->
		new_page_para = parse_fragment_id(location.hash)
		update_page new_page_para
		page_parameters = new_page_para
		update_topbar()
		return
	)
	return
$(document).ready( ->
	register_event_handler()
	page_parameters = parse_fragment_id(location.hash)
	update_category_tag(page_parameters, ->
		expand_post(page_parameters.post)
	)
	update_topbar()
)
