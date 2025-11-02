BUILD_FILES := $(wildcard out/*)
BUILD_DIR := ./out
BUILD_FLAGS := -c Release -o $(BUILD_DIR) -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugSymbols=false
DOTNET := $(shell which dotnet)
DEPENDS := JamDataFinder.csproj $(DOTNET)

all : clean restore publish

clean: $(BUILD_FILES)
	rm -rf $(BUILD_DIR)/*

restore: $(DEPENDS)
	$(DOTNET) restore .

publish: $(DEPENDS)
	$(DOTNET) publish . $(BUILD_FLAGS)

