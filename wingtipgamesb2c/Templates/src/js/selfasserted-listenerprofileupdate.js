$(function () {
	var $marketingConsentedTextInput = $('#extension_MarketingConsented');
	
	$marketingConsentedTextInput.before(function () {
		return '<input id="extension_MarketingConsented_true" name="extension_MarketingConsented_true" type="checkbox"><label for="extension_MarketingConsented_true">' + $marketingConsentedTextInput.prop('placeholder') + '</label>';
	});
	
	var $marketingConsentedCheckboxInput = $('#extension_MarketingConsented_true');
	$marketingConsentedCheckboxInput.prop('checked', $marketingConsentedTextInput.val() === 'true');
	var $marketingFrequencyListItem = $('.self_asserted_listener_profile_update_container #attributeList ul li:nth-child(3)');
	
	if ($marketingConsentedCheckboxInput.prop('checked')) {
		$marketingFrequencyListItem.show();
	}
	
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
