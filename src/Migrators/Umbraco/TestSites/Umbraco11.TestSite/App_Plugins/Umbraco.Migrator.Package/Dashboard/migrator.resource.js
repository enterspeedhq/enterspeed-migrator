angular.module('umbraco.resources')
    .factory('migratorResource', function ($http, umbRequestHelper) {
        return {
            importDocumentTypes() {
                return $http.post("/umbraco/backoffice/api/Migrator/ImportDocumentTypes");
            },
            importData(enterspeedHandle, sectionId) {
                let req = {
                    method: "POST",
                    url: "/umbraco/backoffice/api/Migrator/ImportData",
                    data: {
                        enterspeedHandle: enterspeedHandle,
                        sectionId: sectionId
                    }
                }
                return umbRequestHelper.resourcePromise($http(req), "Failed to get contacts");
            },
        }
    })