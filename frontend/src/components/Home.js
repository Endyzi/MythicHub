import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { TextField, Button, Card, CardContent, Typography, Grid } from "@mui/material";

const Home = () => {
  const [players, setPlayers] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    //fetch top 20 players from the API
    const fetchTopPlayers = async () => {
      try {
        const response = await fetch("http://localhost:5270/api/blizzard/top20"); // Update the endpoint if needed
        if (!response.ok) throw new Error("Failed to fetch top players");
        const data = await response.json();
        setPlayers(data);
      } catch (error) {
        console.error(error);
      }
    };

    fetchTopPlayers();
  }, []);

  const handleSearch = async () => {
    if (searchQuery.trim() === "") return;
    try {
      const response = await fetch(`http://localhost:5270/api/blizzard/search/eu/${searchQuery}`);
      if (!response.ok) throw new Error("Character not found");
      const data = await response.json();
      setSearchResults(Array.isArray(data) ? data : [data]);
    } catch (error) {
      console.error(error);
      setSearchResults([]);
    }
  };

  const handleSelectCharacter = (character) => {
    navigate(`/character/eu/${character.realm}/${character.name}`);
  };

  return (
    <div style={{ padding: "20px", textAlign: "center" }}>
      <Typography variant="h4" gutterBottom>
        Top 20 Players
      </Typography>

      <TextField
        label="Search character..."
        variant="outlined"
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        onKeyDown={(e) => e.key === "Enter" && handleSearch()}
        style={{ marginBottom: "20px", width: "300px" }}
      />
      <Button variant="contained" onClick={handleSearch} style={{ marginLeft: "10px" }}>
        Search
      </Button>

      {/* Searchresult */}
      {searchResults.length > 0 && (
        <div style={{ marginTop: "20px" }}>
          {searchResults.map((char, index) => (
            <Card
              key={index}
              style={{ cursor: "pointer", marginBottom: "10px" }}
              onClick={() => handleSelectCharacter(char)}
            >
              <CardContent>
                <Typography>{`${char.name} - ${char.realm}`}</Typography>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Top 20 Players Grid */}
      <Grid container spacing={3} justifyContent="center">
        {players.map((player, index) => (
          <Grid item key={index} xs={12} sm={6} md={4} lg={3}>
            <Card style={{ backgroundColor: "#1a1a2e", color: "#ffffff" }}>
              <CardContent>
                <Typography variant="h6">{player.name}</Typography>
                <Typography variant="body2">{player.realm}</Typography>
                <Typography variant="body2">{player.class}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </div>
  );
};

export default Home;
