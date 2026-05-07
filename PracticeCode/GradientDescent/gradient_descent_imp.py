import numpy as np

def gradient_descent(x, y, lr=0.01, epochs=3000):
    m, b = 0.0, 0.0  # Initialize parameters

    for epoch in range(epochs):
        y_pred = m * x + b  # Predicted values
        error =  y - y_pred  # Error
        cost = np.mean(error ** 2)  # Mean Squared Error
        dm = -2* np.mean(x * error)  # Gradient w.r.t m
        db = -2 * np.mean(error)  # Gradient w.r.t b
        b -= db * lr  # Update b
        m -= dm * lr
        print(f"m: {m:.4f}, b: {b:.4f}, Epoch {epoch}, Cost: {cost:.4f}")

if __name__ == "__main__":

    # Example data
    x = np.array([1, 2, 3, 4, 5])
    y = np.array([5, 7, 9, 11, 13])
    # Run gradient descent
    gradient_descent(x, y)
    

   