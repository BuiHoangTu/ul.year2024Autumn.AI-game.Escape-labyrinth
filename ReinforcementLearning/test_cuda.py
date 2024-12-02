import torch
print(f"Is cuda available: {torch.cuda.is_available()}")
print(f"There are {torch.cuda.device_count()} devices")
print(f"Current device id: {torch.cuda.current_device()}")
