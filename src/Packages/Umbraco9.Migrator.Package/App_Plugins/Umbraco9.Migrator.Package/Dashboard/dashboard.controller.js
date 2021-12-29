function dashboardMigratorController(notificationService, migratorResource) {
    let vm = this;
    vm.title = "title";

    vm.importDocumentTypes = importDocumentTypes;

    function importDocumentTypes() {
        migratorResource.importDocumentTypes().then(function (result) {
            notificationsService.success("Document types imported");
        }, function (error) {
            notificationsService.error("Something went wrong when importing document types: " + error);
        });
    }
}

angular.module("umbraco").controller("DashboardMigratorController", dashboardMigratorController);
