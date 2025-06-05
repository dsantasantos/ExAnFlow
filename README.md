# ExAnFlow API

This document provides instructions on how to run the ExAnFlow API project using Docker or the dotnet CLI.

## Running with Docker

There are two main ways to run the application using Docker:

### Using `docker-compose` (Recommended)

This is the simplest way to get the application running as it uses the pre-defined configuration in the `docker-compose.yml` file.

1.  **Build and Run:**
    Open your terminal in the root directory of the project and run:
    ```bash
    docker-compose up -d
    ```
    This command will build the Docker image (if it doesn't exist yet or if changes were made) and start the container in detached mode.

2.  **Accessing the application:**
    Once the container is running, the API will be accessible at `http://localhost:8080`.

3.  **Stopping the application:**
    To stop the application, run:
    ```bash
    docker-compose down
    ```

### Using `docker build` and `docker run`

You can also build and run the container manually.

1.  **Build the Docker image:**
    Open your terminal in the root directory of the project and run:
    ```bash
    docker build -t exanflow-api .
    ```
    This command builds the Docker image and tags it as `exanflow-api`.

2.  **Run the Docker container:**
    After the image is built, run the following command:
    ```bash
    docker run -d -p 8080:80 --name exanflow-container exanflow-api
    ```
    - `-d`: Runs the container in detached mode.
    - `-p 8080:80`: Maps port 8080 on your host to port 80 in the container.
    - `--name exanflow-container`: Assigns a name to the container for easier management.

3.  **Accessing the application:**
    The API will be accessible at `http://localhost:8080`.

4.  **Stopping the container:**
    ```bash
    docker stop exanflow-container
    ```

5.  **Removing the container:**
    ```bash
    docker rm exanflow-container
    ```

## Running with dotnet CLI

You can also run the API directly using the .NET CLI.

1.  **Navigate to the API project directory:**
    Open your terminal and navigate to the API's project folder:
    ```bash
    cd Source/ExAnFlow.Api
    ```

2.  **Run the application:**
    Execute the following command:
    ```bash
    dotnet run
    ```
    This command will build the project and start the Kestrel server.

3.  **Accessing the application:**
    Once the application is running, the API will typically be accessible at `http://localhost:5000` (as configured in `Properties/launchSettings.json`). Check the terminal output from `dotnet run` for the exact URL.

    You can usually access the Swagger UI at `http://localhost:5000/swagger`.

**Note:** Ensure you have the .NET 8.0 SDK installed. You might also need to set up any required environment variables or configuration files manually if they are not handled by the project's default settings for local development. The Tesseract OCR dependencies also need to be installed on your system for the OCR functionality to work. Refer to the `Dockerfile` for the dependencies (`tesseract-ocr`, `libleptonica-dev`).
