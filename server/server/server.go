package server

import (
	"log"
	"net/http"

	"tank-tank-battle-server/config"
)

func Start() {
	http.HandleFunc("/", handleHttp)
	http.HandleFunc("/ws", handleWebSocket)

	log.Println("Server running on ", config.ServerPort)
	err := http.ListenAndServe(config.ServerPort, nil)
	if err != nil {
		log.Fatal("Server failed: ", err)
	}
}

func handleHttp(w http.ResponseWriter, r *http.Request) {
	w.Write([]byte("Hello, Tank Game Server!"))
}
