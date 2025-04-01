FROM msc As Build

WORKDIR /app

COPY . .

RUN dotnet Build