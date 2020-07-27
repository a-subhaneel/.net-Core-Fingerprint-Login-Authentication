# .net-Core-Fingerprint-Login-Authentication
 used .net core framework
 used SECUGEN HAMSTER 20 (fingerprint scanner)
 the application registers the user data along with fingerprint scanned hash key and saved in database.
 Upon login authorization. the database key of the user is matched with current logging in user, with the new key.
 Both hash keys are matched in login view page with one of SECUGEN jquery library, which gives the result  for fingerprint match.


IMPORTANT: make sure your browser is not blocking "https://localhost:8443/SGIFPCapture" this site, if it is blocking go to advance and go for unsafe mode. this link is responsible for calling relative data for the fingerprint scan. if this website is blocked finger print capture will fail stating "Check if SGIBIOSRV is running; status = 0".
