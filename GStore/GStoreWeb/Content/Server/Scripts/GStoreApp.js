$(function () {
	$('[data-toggle="popover"]').popover()
})

$(window).resize(function () {
	$('body').css('padding-top', parseInt($('#navbar').css("height")) + 10);
});

$(window).load(function () {
	$('body').css('padding-top', parseInt($('#navbar').css("height")) + 10);
});

(function (i, s, o, g, r, a, m) {
	i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
	(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
	m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
})(window,document,'script','//www.google-analytics.com/analytics.js','ga');

$(document).ready
(function ()
{
	if (googleAnalyticsWebPropertyId == null)
	{
		return;
	}
	if (userName == null)
	{
		ga('create', googleAnalyticsWebPropertyId, 'auto');
	}
	else
	{
		ga('create', googleAnalyticsWebPropertyId, { 'userId': userName });
	}
	ga('require', 'displayfeatures');
	ga('require', 'linkid', 'linkid.js');
	ga('send', 'pageview');
});


function SetFocusToFieldId(fieldId)
{
	$(document).ready(function ()
	{
		$('#' + fieldId).focus();
	})
}

function SetFocusToFirstInput()
{
	$(document).ready(function () {
		var firstInput = $('input[type=text],input[type=email],input[type=password],input[type=radio],input[type=checkbox],textarea,select').filter(':visible:first');
		if (firstInput != null) {
			firstInput.focus();
		}
	})
}

function GaEvent(category, action, label) {
	{
		if (googleAnalyticsWebPropertyId == null)
		{
			return;
		}
		ga('send', 'event', category, action, label);
	}
}

function AddUserMessageInfo(title, msg) {
	AddUserMessage(title, msg, "info");
}
function AddUserMessageWarning(title, msg) {
	AddUserMessage(title, msg, "warning");
}
function AddUserMessageSuccess(title, msg) {
	AddUserMessage(title, msg, "success");
}
function AddUserMessageDanger(title, msg) {
	AddUserMessage(title, msg, "danger");
}

function AddUserMessageBottomInfo(title, msg) {
	AddUserMessageBottom(title, msg, "info");
}
function AddUserMessageBottomWarning(title, msg) {
	AddUserMessageBottom(title, msg, "warning");
}
function AddUserMessageBottomSuccess(title, msg) {
	AddUserMessageBottom(title, msg, "success");
}
function AddUserMessageBottomDanger(title, msg) {
	AddUserMessageBottom(title, msg, "danger");
}

function AddUserMessage(title, msg, type) {
	if ((!type) || (type == "")) {
		type = "info";
	}

	var dateTimeString = DateToString(new Date());

	
	var htmlMsg = "\u003cdiv class=\"alert alert-"
		+ type
		+ "\"\u003e\r\n    \u003ca class=\"close\" data-dismiss=\"alert\"\u003e\u0026times;\u003c/a\u003e\r\n    \u003cstrong\u003e"
		+ title
		+ "\u003c/strong\u003e "
		+ msg.replace(/\n/g, "<br/>\n") + " " + dateTimeString
		+ "\r\n\u003c/div\u003e\r\n";

	var userMessageContainer = document.getElementById("usermessagecontainer");
	if (userMessageContainer)
	{
		$('#usermessagecontainer').append(htmlMsg);
	}
	else
	{
		alert(title + "\n[" + type + " Message]" + "\n" + msg);
		$('#usermessagecontainer').append(htmlMsg);
	}
}

function AddUserMessageBottom(title, msg, type) {
	if ((!type) || (type == "")) {
		type = "info";
	}

	var dateTimeString = DateToString(new Date());

	var htmlMsg = "\u003cdiv class=\"alert alert-"
		+ type
		+ "\"\u003e\r\n    \u003ca class=\"close\" data-dismiss=\"alert\"\u003e\u0026times;\u003c/a\u003e\r\n    \u003cstrong\u003e"
		+ title
		+ "\u003c/strong\u003e "
		+ msg.replace(/\n/g, "<br/>\n") + " " + dateTimeString
		+ "\r\n\u003c/div\u003e\r\n";

	var userMessageBottomContainer = document.getElementById("usermessagebottomcontainer");
	if (userMessageBottomContainer) {
		$('#usermessagebottomcontainer').append(htmlMsg);
	}
	else {
		alert(title + "\n[" + type + " Message]" + "\n" + msg);
	}
}

var announcements = Array();
//var announcementMessage = Array();

function AddAnnouncement(message)
{
	announcements[announcements.length] = message;
	//announcementMessage = message;
}

function ProcessAnnouncements(notifyHub)
{
	var index;
	for (index = 0; index < announcements.length; index++) {
		notifyHub.server.sendMessage(announcements[index]);
	}
	UpdateActiveUsers(notifyHub);
}

function UpdateActiveUsers(notifyHub)
{
	notifyHub.server.activeUsers().done(
		function (value)
		{
			//alert("active users: " + value);
			$('#ActiveUsers').text("Active Users: " + value);
		});
}

function DateToString(dateTime)
{
	
	var a_p = "";
	var curr_hour = dateTime.getHours();
	if (curr_hour < 12)
	{
		a_p = "AM";
	}
	else
	{
		a_p = "PM";
	}
	if (curr_hour == 0)
	{
		curr_hour = 12;
	}
	if (curr_hour > 12)
	{
		curr_hour = curr_hour - 12;
	}

	var curr_min = dateTime.getMinutes();

	curr_min = curr_min + "";

	if (curr_min.length == 1)
	{
		curr_min = "0" + curr_min;
	}

	return (dateTime.getMonth() + 1) + "/" + dateTime.getDate() + "/" + dateTime.getFullYear()
		+ " " + curr_hour + ":" + curr_min + " " + a_p

}

$(document).ready(function () {
	var notifyHub = $.connection.notifyHub;
	notifyHub.client.addNewMessageToPage = function (title, message) {
		//Client function to call when server sends a message
		AddUserMessageInfo(title, message);
		UpdateActiveUsers(notifyHub);
	};

	$.connection.hub.start().done(function () {
		ProcessAnnouncements(notifyHub);
		UpdateActiveUsers(notifyHub);
	})
});

function htmlEncodeNewLine(value) {
	var encodedValue = $('<div />').text(value).html();
	return encodedValue.replace(/\n/g, "<br/>\n");
}

function htmlEncode(value) {
	var encodedValue = $('<div />').text(value).html();
	return encodedValue.replace(/\n/g, "<br/>\n");
}

function GoToUrl(url) {
	document.location.href = url;
}

$(document).ready(function () {
	var inp = $('.input-validation-error:first').get(0);

	if (inp) {
		inp.focus();
	}
});

$(document).ready(function () {
	$(".boxshadowjs").each(function (index, element) {
		ApplyBoxShadow(element);
	});
});

function ApplyBoxShadow(element) {
	$(element).css('width', "95%");
	$(element).css('border', '1px solid silver');
	$(element).css('box-shadow', 'rgba(0, 0, 0, 0.2) 6px 6px');
}

function ValidateUrlName(element)
{
	var value = element.value;
	if (UrlNameIsValid(value))
	{
		return;
	}
	element.value = FixUrlName(value);
}

var invalidUrlNameCharacters = [ '/', '\\', '%', '?', '<', '>', '*', ':', '&', '"', '\'', ' ', '!' ];

function UrlNameIsValid(value)
{
	for (var i = 0; i <= invalidUrlNameCharacters.length; i++)
	{
		if (value.indexOf(invalidUrlNameCharacters[i]) != -1)
		{
			return false;
		}
	}
	return true;
}

function FixUrlName(value)
{
	for (var i = 0; i <= invalidUrlNameCharacters.length; i++)
	{
		value = value.replace(invalidUrlNameCharacters[i], "-");
	}
	if (!UrlNameIsValid(value))
	{
		return FixUrlName(value);
	}
	return value;
}

function UpdateUrlName(idUrlName, idName)
{
	var newName = $('#' + idName).val();
	var newUrlName = FixUrlName(newName);
	$('#' + idUrlName).val(newUrlName);
}
