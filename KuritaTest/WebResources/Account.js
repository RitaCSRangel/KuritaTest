if (typeof KURITA == "undefined") {
    KURITA = {};
}

if (typeof KURITA.Crm == "undefined") {
    KURITA.Crm = {};
}

KURITA.Crm.Account = {

    OnLoad: function (executionContext) {

    },

    DeleteAccount: function (executionContext) {
        var alertString = { confirmButtonLabel: "OK", text: "This account will be deleted, would you like to continue?" };
        Xrm.Navigation.openAlertDialog(alertString).then(
            function (success) {
                var Id = Xrm.Page.data.entity.getId().replace('{', '').replace('}', '');
                Xrm.WebApi.deleteRecord("account", Id).then(
                    function success(result) {
                        Xrm.Utility.alertDialog("Account deleted", null);
                    },
                    function (error) {
                        Xrm.Utility.alertDialog(error.message, null);
                    });
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message, null);
            }
        );
    }
}