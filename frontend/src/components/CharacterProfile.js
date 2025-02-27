import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const CharacterProfile = () => {
    const { region, realm, name } = useParams();
    const [character, setCharacter] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCharacter = async () => {
            try {
                const response = await fetch(`http://localhost:5270/api/blizzard/character/${region}/${realm}/${name}`);
                if (response.ok) {
                    const data = await response.json();
                    setCharacter(data);
                } else {
                    setCharacter(null);
                }
            } catch (error) {
                console.error("Error fetching character details:", error);
                setCharacter(null);
            } finally {
                setLoading(false);
            }
        };

        fetchCharacter();
    }, [region, realm, name]);

    if (loading) return <p>Loading...</p>;
    if (!character) return <p>Character not found.</p>;

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold">{character.name}</h1>
            <p>Faction: {character.faction}</p>
            <p>Race: {character.race.name}</p>
            <p>Class: {character.character_class.name}</p>
            <p>Level: {character.level}</p>
            <p>Item Level: {character.equipped_item_level}</p>
            <img src={character.media.avatar_url} alt={`${character.name}'s Avatar`} />
        </div>
    );
};

export default CharacterProfile;
