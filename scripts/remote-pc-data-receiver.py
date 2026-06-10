import socket

HOST = '0.0.0.0'
PORT = 8080

def main():
    print("🚀 Daiybotics TCP Receiver Started")
    print(f"Listening on {HOST}:{PORT} (all interfaces)\n")

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        server.bind((HOST, PORT))
        server.listen(1)
        
        print("⏳ Waiting for Quest 3S connection...")
        
        try:
            conn, addr = server.accept()
            print(f"✅ Connected by {addr}\n")
            
            buffer = ""
            while True:
                data = conn.recv(2048).decode('ascii', errors='ignore')
                if not data:
                    break
                
                buffer += data
                lines = buffer.split('\n')
                buffer = lines[-1]  # keep partial line
                
                for line in lines[:-1]:
                    line = line.strip()
                    if not line:
                        continue
                    
                    print(f"→ {line}")
                    
                    if line.startswith("GRIPR"):
                        print(f"   🔥 RIGHT GRIP: {line[6:]}")
                    elif line.startswith("GRIPL"):
                        print(f"   🔥 LEFT  GRIP: {line[6:]}")
                    elif line.startswith("C1R"):
                        print(f"   Cube1 Rotation: {line[4:]}")
                    elif "P," in line or "R," in line:
                        print(f"   {line}")
                
                print("-" * 60)

        except KeyboardInterrupt:
            print("\n🛑 Stopped by user")
        except Exception as e:
            print(f"Error: {e}")
        finally:
            conn.close()
            print("Connection closed.")

if __name__ == "__main__":
    main()