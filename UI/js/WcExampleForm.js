(function ($)
{
	$("select:first").focus();

	$(function ()
	{
		$('#NeedRemark').on("click", toggleRemarks);
	});

	$(function ()
	{
		$('button[type=submit]').on("click", validateColId);
	});
})(jQuery);

var toggleRemarks = function ()
{
	if ($("#pnlRemark").hasClass("hide")) $("#pnlRemark").removeClass("hide");
	else $("#pnlRemark").addClass("hide");
};

var validateColId = function (e)
{
	e.preventDefault();
	var id = $('#collocationId').val();
	var entry = $('#Entry').val();
	var entryzht = $('#EntryZht').val();
	var entryzhs = $('#EntryZhs').val();
	var entryjap = $('#EntryJap').val();

	if (id == "0")
	{
		alert('CollocationId is required!');
		$('#collocationId').focus();
	}
	else if (entry == "")
	{
		alert('Entry is required!');
		$('#Entry').focus();
	}
	else if (entryzht == "")
	{
		alert('EntryZht is required!');
		$('#EntryZht').focus();
	}
	else if (entryzhs == "")
	{
		alert('EntryZhs is required!');
		$('#EntryZhs').focus();
	}
	else if (entryjap == "")
	{
		alert('EntryJap is required!');
		$('#EntryJap').focus();
	}
	else
	{
		$('form#ExampleForm').submit();
	}
}