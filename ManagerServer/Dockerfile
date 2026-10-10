FROM mcr.microsoft.com/dotnet/runtime-deps:10.0

RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf

RUN mkdir /opt/manager-server

# Switch to Workstation GC to reduce GC thread count and memory usage which is more appropriate for multiple small containers on a single host.
ENV DOTNET_GCServer=0

# Without this we can't have more than 128 instances on single server
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/?view=aspnetcore-10.0
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

COPY ["./out", "/opt/manager-server"]

# Without this, ManagerServer starts slower in ECS due to a delayed health check error:
# "No connection could be made because the target machine actively refused it."
# It's probably because when using UseStaticFiles() middleware and working directory is set to "/", ASP.NET Core probes directories which we don't care about which causes delay
WORKDIR /opt/manager-server

VOLUME /data

# Defaults for anyone pulling the public image. A "command" in an ECS task
# definition replaces CMD, and command-line arguments override ASPNETCORE_URLS.
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
CMD ["./ManagerServer", "--path", "/data"]
