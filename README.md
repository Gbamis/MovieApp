# MovieApp
System design interview submission
1. Setup Instructions
   - Download the MovieApp apk and install (https://drive.google.com/file/d/1logBxbkN7ZcBEu_AIcD3JkiXxm3g3jUb/view?usp=sharing)
   - Launch app with internet connection
2. Architecture Overview
   - UI screens are managed and controlled using a state machine "UIController". This ensures smooth Entry and Exit screen transistions.
   - RestAPI communication uses a network Service "NetworkManager" to make requests and parse responses.
   - Network responses are Deserialized using JsonUility.
3. Known Issues
   - Search results are filtered to prevent display of results with empty data
4. Possible Improvements
   - UI Pooling of movie card ui items to improve rendering of large query results.
