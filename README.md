# prefix-suffix-bot
Get some vocabulary, and add an unhinged words that you want. Currently support Mastodon API.

## Prerequisite
- .NET CORE 7.0
- (Optional) Nix Package Manager

## How to Install (Nix)
1. Just do this command, Nix help you to download your deps.
```sh
nix-shell dev.nix
```
2. Go to the next section for running the program

## Run the Program
1. Copy or rename `.env.example` to `.env` and edit according to your needs.
    - `PREFIX_POST`: Set prefix on the keyword
    - `SUFFIX_POST`: Set suffix on the keyword
    - `LOOP_LENGTH`: in minute, Interval of the loop in minute. **The limit of this property is 60**.
    - `MASTODON_URI`: The uri of your Mastodon Instance
2. Generate the database first
```
./PrefixSuffixBot generate db
```
3. Import all language to the app
```
./PrefixSuffixBot generate lang id
```
4. Run the program
```
./PrefixSuffixBot
```
5. If you run this program first time, you must validate your authentication to your instance. For this one, I suggest you can do it locally and import the database to your server after verifcation. *Since this is a mini project, I think I didn't want to make to support reverse proxy for verifying remotely, but we'll see for next time.*