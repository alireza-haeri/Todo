FROM mcr.microsoft.com/dotnet/sdk:10.0

# نصب dotnet-ef
RUN dotnet tool install --global dotnet-ef --version 10.0.1 \
    && export PATH="$PATH:/root/.dotnet/tools"

# کپی اسکریپت wait-for-it
COPY wait-for-it.sh /usr/local/bin/wait-for-it.sh
RUN chmod +x /usr/local/bin/wait-for-it.sh

# کپی پروژه‌ها
WORKDIR /src
COPY . .

# تنظیم PATH برای dotnet tools
ENV PATH="$PATH:/root/.dotnet/tools"

# دستور پیش‌فرض: صبر برای SQL و سپس migration
ENTRYPOINT ["/usr/local/bin/wait-for-it.sh", "sqlserver:1433", "--timeout=60", "--strict", "--"]
CMD ["dotnet-ef", "database", "update", "--project", "Todo.Profile", "--startup-project", "Todo.Profile", "--verbose"]
