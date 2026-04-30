I need to implement a full, secure auth system in my project.

A Keycloak server will serve as the main identity provider for the project :
it should manage users, session validity, etc.

All services of this project will, if needed, rely on Keycloak for validating
user identification and validity in an SSO-like fashion.

Specifically :

The Gateway will verify identification and permissions for certain requests. It will
receive the sessions' tokens, verify the validity of the authentication with Keycloak 
before deciding to process the request or not.

The Main Website will have its own local database of user and sessions in order
to use the auth systems, will not allow creating/managing users ; and when a user tries to connect,
it will automatically be redirected to keycloak, which will callback the sessions' and user's infos to the website.
The user infos will be saved a "social account", which will be bound to a "local account" (if it does not currently
exists in the database, a matching local account is automatically created). A local session will be created and managed by the website.
The local session is logically dependant on Keycloak's, so that if Keycloak's user session should be invalidated or has expired (the server will know
it by saving the session's expiration), the website will try to refresh the session with Keycloak. If that does
not work, we consider that the local session should be terminated, and the user will have to relogin before continuing its request.
All this process should be handled by an Auth middleware. 