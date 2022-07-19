FROM python:3.10.5-slim-buster

WORKDIR /flask-server

COPY requirements.txt requirements.txt
RUN python3 -m pip install -r requirements.txt

COPY . .

ENV FLASK_APP=server
CMD [ "python3", "-m" , "flask", "run", "--host=0.0.0.0"]