$(function () {
	var $marketingConsentedTextInput = $('#extension_MarketingConsented');
	
	$marketingConsentedTextInput.before(function () {
		return '<input id="extension_MarketingConsented_true" name="extension_MarketingConsented_true" type="checkbox"><label for="extension_MarketingConsented_true">' + $marketingConsentedTextInput.prop('placeholder') + '</label>';
	});
	
	var $marketingConsentedCheckboxInput = $('#extension_MarketingConsented_true');
	var $marketingFrequencyListItem = $('.self_asserted_player_profile_registration_full_container #attributeList ul li:nth-child(6)');
	
	$marketingConsentedCheckboxInput.click(function () {
		var $this = $(this);
		
		if ($this.is(':checked')) {
			$marketingConsentedTextInput.val('true');
			$marketingFrequencyListItem.show('fast');
		} else {
			$marketingConsentedTextInput.val('false');
			$marketingFrequencyListItem.hide('fast');
		}
	});
});
