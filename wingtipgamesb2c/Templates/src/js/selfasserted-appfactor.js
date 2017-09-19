$(function () {
	var $strongAuthenticationAppQRCodeBitmapImage = $('#strongAuthenticationAppQRCodeBitmapImage');
	var $strongAuthenticationAppQRCodeBitmapTextInput = $('#strongAuthenticationAppQRCodeBitmap');
	$strongAuthenticationAppQRCodeBitmapImage.attr('src', 'data:image/png;base64,' + $strongAuthenticationAppQRCodeBitmapTextInput.val());
	$('#attributeList ul li:nth-child(2)').remove();
});
