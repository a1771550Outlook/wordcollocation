(function ($) {
	$("button[name='btnSearch']").click(function (e) {
		e.preventDefault();
		var wordrequired = $("#wordRequired").val();
		var colposrequired = $("#colPosRequired").val();
		var word = $("input[name='Word']").val();
		var submit = true;
		if (word == '') {
			alert(wordrequired);
			$("input[name='Word']").focus();
			submit = false;
		}
		var id = $("select#ColPosId").val();
		if (id == "0") {
			alert(colposrequired);
			$("select#ColPosId").focus();
			submit = false;
		}

		if (submit) $("form#search-form").submit();
	});
})(jQuery);